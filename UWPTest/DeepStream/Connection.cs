using DeepStreamNet;
using DeepStreamNet.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPTest.DeepStream
{
    class Connection
    {
        static private Connection _instance;
        static private Object _instanceLock = new Object();
        static readonly String NoError = "no error";

        private DeepStreamClient _client;
        private String _lastError = NoError;

        static public Connection Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Connection();
                        }
                    }
                }
                return _instance;
            }
        }

        private Connection()
        {
        }

        private void ResetLastError()
        {
            _lastError = NoError;
        }

        public async Task<bool> ConnectAsync(String host, int port)
        {
            bool result = false;
            ResetLastError();
            Disconnect();

            try
            {
                _client = new DeepStreamClient(host, port, "deepstream", false);
                result = await _client.LoginAsync();
            } 
            catch (Exception ex)
            {
                _lastError = ex.Message;
            }

            return result;
        }

        public async Task<IDeepStreamRecord> GetRecordAsync(String name)
        {
            ResetLastError();

            IDeepStreamRecord result = null;
            if (_client != null)
            {
                try
                {
                    if (await _client.Records.HasAsync(name))
                    {
                        result = await _client.Records.GetRecordAsync(name);
                    }
                    else
                    {
                        _lastError = "Record not exists";
                    }
                }
                catch (Exception ex)
                {
                    _lastError = ex.Message;
                }
            }
            else
            {
                _lastError = "Not Connected";
            }

            return result;
        }

        public String LastError
        {
            get
            {
                return _lastError;
            }
        }

        public void Disconnect()
        {
            if (_client != null)
            {
                _client.Dispose();
            }
        }
    }
}
