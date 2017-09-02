using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOrganizer.Validators
{
    public interface IDirectoryValidator
    {
        bool Validate(string directoryPath, out ICollection<string> validationErrors);
    }
}
