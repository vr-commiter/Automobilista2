using TrueGearSDK;


namespace MyTrueGear
{
    public class TrueGearMod
    {
        private static TrueGearPlayer _player = null;

        public TrueGearMod() 
        {
            _player = new TrueGearPlayer("1066890","Automobilista 2");
            _player.Start();
        }


        public void Play(string Event)
        { 
            _player.SendPlay(Event);
        }

    }
}
