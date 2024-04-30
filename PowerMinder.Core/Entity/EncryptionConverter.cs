using EntityFrameworkCore.EncryptColumn.Interfaces;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;


namespace PowerMinder.Core.Entity
{
    internal class EncryptionConverter : ValueConverter<string, string>
    {
        public EncryptionConverter(IEncryptionProvider encryptionProvider, ConverterMappingHints mappingHints = null)
            : base((Expression<Func<string, string>>)((string x) => encryptionProvider.Encrypt(x)), (Expression<Func<string, string>>)((string x) => encryptionProvider.Decrypt(x)), mappingHints)
        {
        }
    }
}
