using System;
using System.Collections.Generic;

public class ClientStorage {
    private Dictionary<int, Guid> _clientDictionary = new Dictionary<int, Guid>();
    private static ClientStorage _instance = null;
    public Guid? authServerConnection { get; private set; } = null;

    public static ClientStorage Instance {
        get
        {
            if (_instance == null)
                _instance = new ClientStorage();
            return _instance;
        }
    }

    public void AddClient(int clientId, Guid connectionId, bool isAuth) {
        if (_clientDictionary.ContainsKey(clientId)) {
            _clientDictionary.Remove(clientId);
        }
        _clientDictionary.Add(clientId, connectionId);
        if (isAuth) {
            authServerConnection = connectionId;
        }
    }

    public void RemoveClient(int clientId, bool isAuth) {
        if (_clientDictionary.ContainsKey(clientId)) {
            _clientDictionary.Remove(clientId);
        }
        if (isAuth)
            authServerConnection = null;
    }

    public Guid? GetConnectionIdFromClientId(int clientId) {
        if (_clientDictionary.ContainsKey(clientId)) {
            return _clientDictionary[clientId];
        }
        return null;
    }
}
