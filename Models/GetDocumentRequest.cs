using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlobDocument.Models
{
    //public class GetDocumentRequest
    //{
    //    public int DocumentID { get; set; }
    //    public string Type { get; set; }
    //}
    public class Response<T>
    {
        public T Result { get; set; }

        public IDictionary<string, string[]> Errors { get; set; }

    }
}
