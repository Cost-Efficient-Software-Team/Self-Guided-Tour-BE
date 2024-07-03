using SelfGuidedTours.Core.Models.Dto;
using SelfGuidedTours.Infrastructure.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfGuidedTours.Core.Contracts
{
    public interface ILandmarkService
    {
        Task<ICollection<Landmark>> CreateLandmarskForTourAsync(ICollection<LandmarkCreateTourDTO> landmarksDto,  Tour tour);
    }
}
