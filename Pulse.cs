using NAudio.Wave;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BoseCineMatePulse
{
    public class Igniter : IDisposable
    {
        private CancellationTokenSource? _cts;
        private Task? _timerTask;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(10);
        private const int SampleRate = 48000;
        private const float Duration = 5f;
        private const float FadeInDuration = 2f;
        private const float FadeOutDuration = 2f;

        private WaveOutEvent? _waveOut;
        private PureLowFrequencyProvider? _waveProvider;

        public Igniter()
        {
            _timerTask = DoStart(_interval);
        }

        private async Task DoStart(TimeSpan interval)
        {
            _cts = new CancellationTokenSource();
            try
            {
                using var timer = new PeriodicTimer(interval);
                do
                {
                    await PlaySmoothSoundAsync();
                }
                while (await timer.WaitForNextTickAsync(_cts.Token));
            }
            catch (OperationCanceledException) { }
            finally
            {
                DisposeAudio();
            }
        }

        private async Task PlaySmoothSoundAsync()
        {
            try
            {
                DisposeAudio();

                Debug.WriteLine("Starting sound generation.");

                _waveProvider = new PureLowFrequencyProvider(SampleRate, Duration, FadeInDuration, FadeOutDuration);

                _waveOut = new WaveOutEvent();
                _waveOut.Init(_waveProvider);
                _waveOut.Play();

                Debug.WriteLine("Sound playback started");

                await Task.Delay(TimeSpan.FromSeconds(Duration + 0.1));

                _waveOut.Stop();
                Debug.WriteLine("Sound playback stopped");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error playing sound: {ex.Message}");
            }
            finally
            {
                DisposeAudio();
            }
        }

        private void DisposeAudio()
        {
            _waveOut?.Dispose();
            _waveOut = null;
            _waveProvider = null;
        }

        public async Task IgniteAsync()
        {
            Debug.WriteLine("Manual Pulse triggered");
            await PlaySmoothSoundAsync();
        }

        public void Dispose()
        {
            if (_cts is not null)
            {
                _cts.Cancel();
                _timerTask?.Wait();
                _cts.Dispose();
            }
            DisposeAudio();
            GC.SuppressFinalize(this);
        }
    }

    public class PureLowFrequencyProvider : ISampleProvider
    {
        private readonly WaveFormat _waveFormat;
        private readonly float[] _buffer;
        private int _position;

        public WaveFormat WaveFormat => _waveFormat;

        public PureLowFrequencyProvider(int sampleRate, float duration, float fadeInDuration, float fadeOutDuration)
        {
            _waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 1);
            int totalSamples = (int)(duration * sampleRate);
            _buffer = new float[totalSamples];

            int fadeInSamples = (int)(fadeInDuration * sampleRate);
            int fadeOutSamples = (int)(fadeOutDuration * sampleRate);
            double twoPi = 2 * Math.PI;

            float[] frequencies = { 1f, 2f, 3f };
            float[] amplitudes = { 0.5f, 0.3f, 0.2f };

            for (int i = 0; i < totalSamples; i++)
            {
                double time = (double)i / sampleRate;
                double envelope = GetUltraSmoothEnvelope(i, totalSamples, fadeInSamples, fadeOutSamples);
                double combinedWave = 0;

                for (int j = 0; j < frequencies.Length; j++)
                {
                    combinedWave += amplitudes[j] * Math.Sin(twoPi * frequencies[j] * time);
                }

                _buffer[i] = (float)(envelope * combinedWave * 0.2); // Reduced overall volume
            }

            // Ensure the buffer starts and ends at exactly zero
            for (int i = 0; i < 1000; i++)
            {
                _buffer[i] *= i / 1000f;
                _buffer[totalSamples - 1 - i] *= i / 1000f;
            }
        }

        private double GetUltraSmoothEnvelope(int sample, int totalSamples, int fadeInSamples, int fadeOutSamples)
        {
            if (sample < fadeInSamples)
            {
                double t = (double)sample / fadeInSamples;
                return SmoothStep(t);
            }
            else if (sample > totalSamples - fadeOutSamples)
            {
                double t = (double)(totalSamples - sample) / fadeOutSamples;
                return SmoothStep(t);
            }
            else
            {
                return 1.0;
            }
        }

        private double SmoothStep(double t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = 0;
            while (samplesRead < count && _position < _buffer.Length)
            {
                buffer[offset + samplesRead] = _buffer[_position];
                _position++;
                samplesRead++;
            }
            return samplesRead;
        }
    }
}