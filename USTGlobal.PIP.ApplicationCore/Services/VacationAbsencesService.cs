using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class VacationAbsencesService : IVacationAbsencesService
    {
        private readonly IVacationAbsencesRepository vacationAbsencesRepository;

        public VacationAbsencesService(IVacationAbsencesRepository vacationAbsencesRepository)
        {
            this.vacationAbsencesRepository = vacationAbsencesRepository;
        }

        public async Task<VacationAbsencesParentDTO> GetVacationAbsences(int pipSheetId)
        {
            return await vacationAbsencesRepository.GetVacationAbsences(pipSheetId);
        }

        public async Task SaveVacationAbsencesData(string userName, VacationAbsencesParentDTO vacationAbsencesParentDTO)
        {
            VacationAbsencesDTO vacationAbsencesDTO = new VacationAbsencesDTO
            {
                PIPSheetId = vacationAbsencesParentDTO.PIPSheetId,
                Amount = vacationAbsencesParentDTO.Amount,
                IsOverride = vacationAbsencesParentDTO.IsOverride,
                IsPercent = vacationAbsencesParentDTO.IsPercent,
                LostRevenue = vacationAbsencesParentDTO.LostRevenue,
                IsOverrideUpdated = vacationAbsencesParentDTO.IsOverrideUpdated,
            };

            IList<ProjectPeriodTotalDTO> projectPeriodTotalDTO = vacationAbsencesParentDTO.PeriodLostRevenue.Select(x => new ProjectPeriodTotalDTO
            {
                BillingPeriodId = x.BillingPeriodId,
                ProjectPeriodId = x.ProjectPeriodId,
                LostRevenue = x.LostRevenue,
                PipSheetId = vacationAbsencesDTO.PIPSheetId
            }).ToList();

            await vacationAbsencesRepository.SaveVacationAbsencesData(userName, vacationAbsencesDTO, projectPeriodTotalDTO);
        }

        public async Task<VacationAbsencesParentDTO> CalculateVacationAbsences(int pipSheetId, string userName)
        {
            VacationAbsencesParentDTO vacationAbsences = await vacationAbsencesRepository.GetVacationAbsences(pipSheetId);
            if (vacationAbsences != null)
            {
                if (!vacationAbsences.IsMarginSet)
                {
                    if (!vacationAbsences.IsOverride)
                    {
                        vacationAbsences.IsOverrideUpdated = false;
                        // Calculating Lost Revenue
                        if (vacationAbsences.IsPercent)
                            vacationAbsences.LostRevenue = (vacationAbsences.Amount / 100) * vacationAbsences.TotalRevenue;
                        else
                            vacationAbsences.LostRevenue = vacationAbsences.Amount;

                        //Calculating Period-wise Lost Revenue
                        for (int i = 0; i < vacationAbsences.PeriodLostRevenue.Count; i++)
                        {
                            vacationAbsences.PeriodLostRevenue[i].LostRevenue = (vacationAbsences.PeriodLostRevenue[i].Revenue * vacationAbsences.LostRevenue ?? 0)
                                / (vacationAbsences.TotalRevenue == 0 ? 1 : vacationAbsences.TotalRevenue);
                        }
                    }
                    else
                    {
                        // Case when Override = ON
                        if (vacationAbsences.IsPercent)
                        {
                            decimal? overrideAmount = 0;
                            foreach (var period in vacationAbsences.PeriodLostRevenue)
                            {
                                overrideAmount += period.LostRevenue;
                            }

                            if (Math.Abs((Math.Abs(overrideAmount ?? 0)) - Math.Abs(Math.Round(((vacationAbsences.Amount / 100) * vacationAbsences.TotalRevenue) ?? 0))) > 1)
                            {
                                if (vacationAbsences.TotalRevenue >= 0)
                                {
                                    vacationAbsences.LostRevenue = (vacationAbsences.Amount / 100) * vacationAbsences.TotalRevenue;
                                    vacationAbsences.IsOverrideUpdated = true;
                                }
                                else
                                {
                                    // Case when Total Revenue is negative, so on Vacation Absences tab the value of Lost Revenue needs to be calculated to be default with No scenario
                                    vacationAbsences.IsOverrideUpdated = false;
                                    vacationAbsences.LostRevenue = (vacationAbsences.Amount / 100) * vacationAbsences.TotalRevenue;

                                    //Calculating Period-wise Lost Revenue
                                    for (int i = 0; i < vacationAbsences.PeriodLostRevenue.Count; i++)
                                    {
                                        vacationAbsences.PeriodLostRevenue[i].LostRevenue = (vacationAbsences.PeriodLostRevenue[i].Revenue * vacationAbsences.LostRevenue ?? 0)
                                            / (vacationAbsences.TotalRevenue == 0 ? 1 : vacationAbsences.TotalRevenue);
                                    }
                                }
                            }
                            else
                            {
                                vacationAbsences.IsOverrideUpdated = false;
                            }
                        }
                        else
                        {
                            // Case when Override = ON and Amount is inserted
                            if (vacationAbsences.TotalRevenue < 0)
                            {
                                vacationAbsences.LostRevenue = vacationAbsences.Amount;

                                //Calculating Period-wise Lost Revenue
                                for (int i = 0; i < vacationAbsences.PeriodLostRevenue.Count; i++)
                                {
                                    vacationAbsences.PeriodLostRevenue[i].LostRevenue = (vacationAbsences.PeriodLostRevenue[i].Revenue * vacationAbsences.LostRevenue ?? 0)
                                        / (vacationAbsences.TotalRevenue == 0 ? 1 : vacationAbsences.TotalRevenue);
                                }
                            }
                        }
                    }
                }
            }
            vacationAbsences.Amount = (vacationAbsences.Amount ?? 0);
            return vacationAbsences;
        }
    }
}
