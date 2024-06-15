using NAudio.CoreAudioApi;

namespace ThLaunchSite.Game
{
    internal class GameAudio
    {
        public static float GetGameProcessAudioVolume(int gameProcessId)
        {
            MMDeviceEnumerator enumerator = new();
            MMDevice defaultAudioDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
            if (defaultAudioDevice != null)
            {
                AudioSessionManager audioSessionManager = defaultAudioDevice.AudioSessionManager;
                SessionCollection sessionsCollection = audioSessionManager.Sessions;

                for (int i = 0; i < sessionsCollection.Count; i++)
                {
                    AudioSessionControl session = sessionsCollection[i];
                    uint sessionId = session.GetProcessID;

                    if (sessionId == gameProcessId)
                        return session.SimpleAudioVolume.Volume;
                }

                return 0;
            }
            else
            {
                return 0;
            }
        }

        public static void SetGameProcessAudioVolume(int gameProcessId, float volume)
        {
            MMDeviceEnumerator enumerator = new();
            MMDevice defaultAudioDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
            if (defaultAudioDevice != null)
            {
                AudioSessionManager audioSessionManager = defaultAudioDevice.AudioSessionManager;
                SessionCollection sessionsCollection = audioSessionManager.Sessions;

                for (int i = 0; i < sessionsCollection.Count; i++)
                {
                    AudioSessionControl session = sessionsCollection[i];
                    uint sessionId = session.GetProcessID;

                    if (sessionId == gameProcessId)
                    {
                        session.SimpleAudioVolume.Volume = volume;
                        break;
                    }
                }
            }
        }
    }
}
