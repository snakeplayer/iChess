/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServer
 * Project description: A local network chess game. 
 * File: IObserverWindow.cs
 * File description: An interface used to apply the Observer-Subject design pattern.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChessServer
{
    public interface IObserverWindow
    {
        void UpdateView();
    }
}
