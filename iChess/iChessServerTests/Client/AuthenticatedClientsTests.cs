/*
 * Author: Benoit CHAUCHE
 * Date: 2018
 * Project: iChessServerTests
 * Project description: A local network chess game. 
 * File: AuthenticatedClientsTests.cs
 * File description: Handles all authenticated clients on the server. (UNIT TESTS CLASS)
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using iChessServer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkCommsDotNet.Connections;

namespace iChessServer.Client.Tests
{
    [TestClass()]
    public class AuthenticatedClientsTests
    {
        [TestMethod()]
        public void AuthenticatedClientsTest()
        {
            // Arrange
            AuthenticatedClients authClients;

            // Act
            authClients = new AuthenticatedClients();

            // Assert
            Assert.AreNotEqual(null, authClients);
        }

        [TestMethod()]
        public void AddClientTest()
        {
            // Arrange
            AuthenticatedClients authClients = new AuthenticatedClients();

            // Act
            //authClients.AddClient();

            // Assert
            Assert.AreNotEqual(null, authClients);
        }

        [TestMethod()]
        public void RemoveClientTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RemoveClientTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ContainsConnectionTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ContainsUsernameTest()
        {
            Assert.Fail();
        }
    }
}