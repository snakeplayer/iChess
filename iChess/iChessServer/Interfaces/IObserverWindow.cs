/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: IObserverWindow.cs
 * File description: An interface used to apply the Observer-Subject design pattern.
 */

namespace iChessServer
{
    /// <summary>
    /// An interface used to apply the Observer-Subject design pattern.
    /// </summary>
    public interface IObserverWindow
    {
        /// <summary>
        /// Updates the user interface.
        /// </summary>
        void UpdateView();
    }
}
