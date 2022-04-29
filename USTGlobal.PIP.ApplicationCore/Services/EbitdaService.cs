using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class EbitdaService : IEbitdaService
    {
        private readonly IEbitdaRepository ebitdaRepository;

        public EbitdaService(IEbitdaRepository ebitdaRepository)
        {
            this.ebitdaRepository = ebitdaRepository;
        }

        public async Task<List<EbitdaDTO>> GetEbitdaAndStandardOverhead(int pipSheetId)
        {
            return await this.ebitdaRepository.GetEbitdaAndStandardOverhead(pipSheetId);
        }

        public async Task UpdateEbitda(string userName, List<EbitdaDTO> ebitdadata)
        {
            if (ebitdadata.Count > 0)
            {
                bool isOverridenValueLessThanRefUSD = false;
                foreach (EbitdaDTO ebitdaDTO in ebitdadata)
                {
                    if (Math.Round(ebitdaDTO.RefUSD, 2) > Math.Round(ebitdaDTO.OverheadAmount ?? 0, 2))
                    {
                        isOverridenValueLessThanRefUSD = true;
                        break;
                    }
                }
                await this.ebitdaRepository.UpdateEbitda(userName, ebitdadata, isOverridenValueLessThanRefUSD);
            }            
        }

        public List<EbitdaDTO> CreateEbitdaObject(int pipSheetId, List<EbitdaDTO> ebitdaDTO)
        {
            List<EbitdaDTO> listEbitda = new List<EbitdaDTO>();
            EbitdaDTO ebitda = null;

            ebitdaDTO.ForEach(ebitdaObj =>
            {
                ebitda = new EbitdaDTO();
                ebitda.PipSheetId = pipSheetId;
                ebitda.RefUSD = ebitdaObj.RefUSD;
                ebitda.EbitdaSeatCost = ebitdaObj.EbitdaSeatCost;
                ebitda.IsStdOverheadOverriden = ebitdaObj.IsStdOverheadOverriden;
                ebitda.LocationId = ebitdaObj.LocationId;
                ebitda.LocationName = ebitdaObj.LocationName;
                ebitda.OverheadAmount = ebitdaObj.IsStdOverheadOverriden ? ebitdaObj.OverheadAmount : ebitdaObj.RefUSD;
                ebitda.SharedSeatsUsePercent = ebitdaObj.SharedSeatsUsePercent != null ? ebitdaObj.SharedSeatsUsePercent : 100;
                listEbitda.Add(ebitda);
            });
            return listEbitda;
        }
    }
}
