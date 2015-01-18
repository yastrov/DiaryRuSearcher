using System;

namespace DiaryAPI
{
        [Serializable]
        public class DiaryAPIClientException : Exception
        {
            public DiaryAPIClientException() { }
            public DiaryAPIClientException(string message) : base(message) { }
            public DiaryAPIClientException(string message, Exception inner) : base(message, inner) { }
            protected DiaryAPIClientException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }
}

