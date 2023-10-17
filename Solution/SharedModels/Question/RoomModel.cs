using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Question
{
    public record class RoomModel
    {
        public string Name { get; init; }
        public string Id { get; init; }
    }
}
