using System.Collections.Generic;
using UnityEngine;

public class TokenCarrier : MonoBehaviour
{
    PackToken packToken;

    void Awake()
    {
        packToken = new();
    }

    public void AddStatusToken(StatusToken st)
    {
        packToken.AddStatusToken(st);
    }

    public List<StatusToken> GetTokenList()
    {
        return packToken.GetTokenList();
    }

    public void SetTokenList(List<StatusToken> statusTokens)
    {
        packToken.SetTokenList(statusTokens);
    }
}
