using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Question
{
    public record class RoomTransferModel
    {
        public int Id { get; init; }
        public string Name { get; init; }
    }
}
