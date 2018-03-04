using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YalitApi.Models;

namespace YalitApi.Services
{
    public interface IRoomService
    {
        Task<Room> GetRoomAsync(
            Guid id,
            CancellationToken ct);
    }
}
