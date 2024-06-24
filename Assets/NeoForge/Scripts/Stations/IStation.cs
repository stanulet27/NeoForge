namespace NeoForge.Stations
{
    public interface IStation
    {
        /// <summary>
        /// Will be called when the player enters the station to perform any necessary setup.
        /// </summary>
        public void EnterStation();
        
        /// <summary>
        /// Will be called when the player exits the station to perform any necessary cleanup.
        /// </summary>
        public void ExitStation();
    }
}