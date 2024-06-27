using System.Collections.Generic;
using System.Linq;

public class Validator
{
    private ConversationDataHandler _dataHandler;

    public Validator(ConversationDataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public bool ValidateLeadsTo(int currentNodeIndex)
    {
        var conversationData = _dataHandler.ConversationDataSOList[currentNodeIndex].Data;
        if (conversationData == null)
        {
            return false;
        }

        var validIDs = new HashSet<string>();
        foreach (var dataSO in _dataHandler.ConversationDataSOList)
        {
            validIDs.Add(dataSO.Data.ID);
        }

        return conversationData.LeadsTo.All(leadsTo => validIDs.Contains(leadsTo.NextID));
    }
}