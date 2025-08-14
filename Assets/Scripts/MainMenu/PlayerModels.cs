using System;

[Serializable]
public class Player
{
    public int Id;
    public string Username;
    public string CreatedDate;
}

[Serializable]
public class PlayerList
{
    public Player[] players;
}

