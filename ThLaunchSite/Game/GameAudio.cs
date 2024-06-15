using NAudio.CoreAudioApi;

namespace ThLaunchSite.Game
{
    internal class GameAudio
    {
        public static float GetGameProcessAudioVolume(string gameProcessName)
        {
            Process gameProcess = Process.GetProcessesByName(gameProcessName)[0];

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

                    if (sessionId == gameProcess.Id)
                        return session.SimpleAudioVolume.Volume;
                }

                return 0;
            }
            else
            {
                return 0;
            }
        }

        public static void SetGameProcessAudioVolume(string gameProcessName, float volume)
        {
            Process gameProcess = Process.GetProcessesByName(gameProcessName)[0];

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

                    if (sessionId == gameProcess.Id)
                    {
                        session.SimpleAudioVolume.Volume = volume;
                        break;
                    }
                }
            }
        }
    }
}
