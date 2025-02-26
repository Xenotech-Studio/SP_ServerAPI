using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !SERVERAPI_NOT_PRODUCT
using Union.Framework;
using XingHan;
#endif


namespace DataSystem.Http
{
    public partial class MultiplayerManager
    {
        private bool _needReestablishHost = false;
        private Coroutine _reestablishHostCoroutine;
        
        private void ReestablishHostUpdate()
        {
            if (_needReestablishHost)
            {
                _needReestablishHost = false;
                _reestablishHostCoroutine = StartCoroutine(ReestablishHostCoroutine());
            }
        }
        
        private string _pending_host_uuid = "unknown";
        
        private IEnumerator ReestablishHostCoroutine()
        {
            _pending_host_uuid = _uuid;
            
            // add listener to ServerAPI
            ServerAPI.AddListener("reestablish_host", this.ReestablishListener);
            
            // wait for delay
            yield return new WaitForSeconds(1);
            
            // if host is still unknown, send message to all
            ServerAPI.Send("reestablish_host", $"{_uuid}:{_pending_host_uuid}");
            
            // wait for delay
            yield return new WaitForSeconds(1);
            
            // confirm the final host
            _host_uuid = _pending_host_uuid;
            Debug.Log("Host changed to " + _host_uuid);
            
            // remove listener from ServerAPI
            ServerAPI.RemoveListener("reestablish_host", this.ReestablishListener);
        }
        
        public void ReestablishListener(MessageInfo info)
        {
            string[] splitMessage = info.Message.Split(":");
            string senderUuid = splitMessage[0];
            string suggestedHost = splitMessage[1];
            
            if (senderUuid != _uuid)
            {
                // compare strings who is greater using ComapreOrdinal
                if (string.CompareOrdinal(suggestedHost, _pending_host_uuid) > 0)
                {
                    _pending_host_uuid = senderUuid;
                }
            }
        }
    }
}