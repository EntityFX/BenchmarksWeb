using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using EntityFx.BenchmarkDb.Contracts;
using EntityFx.BenchmarkDb.Contracts.Cpu;

namespace EntityFx.BenchmarkDb.DataAccess
{
    public class CpuRepository : SqliteRepositoryBase, ICpuRepository
    {
        public void Create(Cpu cpu)
        {
            try
            {
                var model = MapEntity(cpu);
                model.CreateDateTime = DateTime.Now;
                var id = Connection.Insert<CpuEntity>(model);
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public async void Delete(int id)
        {
            var result = await ReadByIdAsync(id);
            if (result == null)
            {
                return;
            }

            Connection.Delete(result);
        }

        public async Task<IEnumerable<Cpu>> ReadAsync(CpuFilter filter)
        {
            var result = await ReadEntitiesAsync(filter);

            return result == null ? null : result.Select(MapModel);
        }

        private async Task<IEnumerable<CpuEntity>> ReadEntitiesAsync(CpuFilter filter)
        {

            var queryBuilder = new SqlBuilder()
             .Select(nameof(CpuEntity.Id))
             .Select(nameof(CpuEntity.Model))
             .Select(nameof(CpuEntity.Manufacturer))
             .Select(nameof(CpuEntity.Category))
             .Select(nameof(CpuEntity.InstructionSet))
             .Select(nameof(CpuEntity.ClockInMhz))
             .Select(nameof(CpuEntity.Cores))
             .Select(nameof(CpuEntity.Threads));


            if (filter != null)
            {
                var parameters = new DynamicParameters();

                if (!string.IsNullOrEmpty(filter.SearchString))
                {
                    filter.SearchString = filter.SearchString.Replace(" ", "*");
                    parameters.Add("@SearchString", filter.SearchString + "%");
                    queryBuilder.Where($"{nameof(CpuEntity.Model)} LIKE @SearchString OR {nameof(CpuEntity.Name)} LIKE @SearchString",
                        parameters);
                }
                else
                {
                    AddFilter(filter.ByManufacturer, nameof(CpuEntity.Manufacturer), parameters, queryBuilder);
                    AddFilter(filter.ByCategory, nameof(CpuEntity.Category), parameters, queryBuilder);
                    AddFilter(filter.ByMicroArchitecture, nameof(CpuEntity.MicroArchitecture), parameters, queryBuilder);
                    AddFilter(filter.ByInstructionSet, nameof(CpuEntity.InstructionSet), parameters, queryBuilder);
                }



                if (!string.IsNullOrEmpty(filter.OrderBy))
                {
                    parameters.Add($"@Order", filter.OrderBy);
                    queryBuilder.OrderBy("@Order ASC", parameters);
                }
            }

            var builderTemplate = queryBuilder.AddTemplate("SELECT /**select**/ FROM CpuEntity /**where**/ /**orderby**/");

            var result = await Connection.QueryAsync<CpuEntity>(builderTemplate.RawSql, builderTemplate.Parameters);

            return result;
        }


        public async Task<Cpu> ReadByIdAsync(int cpuId)
        {
            var result = await ReadEntityByIdAsync(cpuId);

            return result == null ? null : MapModel(result);
        }

        private async Task<CpuEntity> ReadEntityByIdAsync(int cpuId)
        {
            var sqlQuery = "SELECT * FROM CpuEntity Where Id = @Id";

            var result = await Connection.QuerySingleOrDefaultAsync<CpuEntity>(sqlQuery, new { Id = cpuId });

            return result;
        }

        private Cpu MapModel(CpuEntity cpuEntity)
        {
            return new Cpu()
            {
                Id = cpuEntity.Id,
                Name = cpuEntity?.Name,
                Model = cpuEntity?.Model,
                Manufacturer = cpuEntity?.Manufacturer,
                Description = cpuEntity?.Description,

                Category = cpuEntity?.Category,
                Family = cpuEntity?.Family,
                Stepping = cpuEntity?.Stepping,
                Revision = cpuEntity?.Revision,
                ModelNumber = cpuEntity?.ModelNumber,

                Specs = new CommonSpecs()
                {
                    Cores = cpuEntity?.Cores ?? 1,
                    Threads = cpuEntity?.Threads ?? 1,
                    MicroArchitecture = cpuEntity?.MicroArchitecture,
                    InstructionSet = cpuEntity?.InstructionSet,
                    ClockInMhz = cpuEntity?.ClockInMhz,
                    BusInMhz = cpuEntity?.BusInMhz,
                    Multiplier = cpuEntity?.Multiplier,
                    Features = cpuEntity?.Features
                },

                Cache = new Cache()
                {
                    L1Data = cpuEntity.CacheL1DSizeKBytes != null ? new CacheItem()
                    {
                        CacheAssociativity = cpuEntity?.CacheL1DCacheAssociativity,
                        SizeKBytes = cpuEntity?.CacheL1DSizeKBytes,
                        LineSizeInBytes = cpuEntity?.CacheL1DLineSizeInBytes,
                        IsShared = cpuEntity?.CacheL1DIsShared,
                        Details = cpuEntity?.CacheL1DDetails,
                    } : null,
                    L1Instruction = cpuEntity.CacheL1ISizeKBytes != null ? new CacheItem()
                    {
                        CacheAssociativity = cpuEntity?.CacheL1ICacheAssociativity,
                        SizeKBytes = cpuEntity?.CacheL1ISizeKBytes,
                        LineSizeInBytes = cpuEntity?.CacheL1ILineSizeInBytes,
                        IsShared = cpuEntity?.CacheL1IIsShared,
                        Details = cpuEntity?.CacheL1IDetails,
                    } : null,
                    L2 = cpuEntity.CacheL2SizeKBytes != null ? new CacheItem()
                    {
                        CacheAssociativity = cpuEntity?.CacheL2CacheAssociativity,
                        SizeKBytes = cpuEntity?.CacheL2SizeKBytes,
                        LineSizeInBytes = cpuEntity?.CacheL2LineSizeInBytes,
                        IsShared = cpuEntity?.CacheL2IsShared,
                        Details = cpuEntity?.CacheL2Details,
                    } : null,
                    L3 = cpuEntity.CacheL3SizeKBytes != null ? new CacheItem()
                    {
                        CacheAssociativity = cpuEntity?.CacheL3CacheAssociativity,
                        SizeKBytes = cpuEntity?.CacheL3SizeKBytes,
                        LineSizeInBytes = cpuEntity?.CacheL3LineSizeInBytes,
                        IsShared = cpuEntity?.CacheL3IsShared,
                        Details = cpuEntity?.CacheL3Details,
                    } : null,
                    L4 = cpuEntity.CacheL4SizeKBytes != null ? new CacheItem()
                    {
                        CacheAssociativity = cpuEntity?.CacheL4CacheAssociativity,
                        SizeKBytes = cpuEntity?.CacheL4SizeKBytes,
                        LineSizeInBytes = cpuEntity?.CacheL4LineSizeInBytes,
                        IsShared = cpuEntity?.CacheL4IsShared,
                        Details = cpuEntity?.CacheL4Details,
                    } : null
                },
                MemorySpecs = cpuEntity.MemoryMemoryType != null ? new MemorySpecs()
                {
                    BandwidthInMbPerSec = cpuEntity?.MemoryBandwidthInMbPerSec,
                    Channels = cpuEntity?.MemoryChannels,
                    Controllers = cpuEntity?.MemoryControllers,
                    EccOnly = cpuEntity?.MemoryEccOnly,
                    Details = cpuEntity?.MemoryDetails,
                    MaxMemorySizeInMb = cpuEntity?.MemoryMaxMemorySizeInMb,
                    MemoryType = cpuEntity?.MemoryMemoryType,
                } : null,
                Crystal = cpuEntity.CrystalArea != null ? new CrystalParameters()
                {
                    HeightMm = cpuEntity.CrystalHeightMm,
                    WidthMm = cpuEntity.CrystalWidthMm,
                    Area = cpuEntity.CrystalArea,
                    TDP = cpuEntity.TDP,
                    TransistorsCount = cpuEntity.TransistorsCount,
                    ProcessInNm = cpuEntity.ProcessInNm,
                } : null,
                Package = cpuEntity.PackageArea != null ? new CrystalParameters()
                {
                    HeightMm = cpuEntity.PackageHeightMm,
                    WidthMm = cpuEntity.PackageWidthMm,
                    Area = cpuEntity.PackageArea,
                } : null,
            };
        }

        private CpuEntity MergeEntity(CpuEntity source, CpuEntity destination)
        {
            destination.Model = source.Model ?? destination.Model;
            destination.Name = source.Name ?? destination.Name;
            destination.Manufacturer = source.Manufacturer ?? destination.Manufacturer;
            destination.Description = source?.Description ?? destination.Description;
            destination.Cores = source.Cores ?? destination.Cores;
            destination.Threads = source.Threads ?? destination.Cores;
            destination.Category = source.Category ?? destination.Category;
            destination.MicroArchitecture = source.MicroArchitecture ?? destination.MicroArchitecture;
            destination.InstructionSet = source.InstructionSet ?? destination.InstructionSet;
            destination.ClockInMhz = source.ClockInMhz ?? destination.ClockInMhz;
            destination.BusInMhz = source.BusInMhz ?? destination.BusInMhz;
            destination.Multiplier = source.Multiplier ?? destination.Multiplier;

            destination.Family = source.Family ?? destination.Family;
            destination.Stepping = source.Stepping ?? destination.Stepping;
            destination.Revision = source.Revision ?? destination.Revision;
            destination.ModelNumber = source.ModelNumber ?? destination.ModelNumber;

            destination.CacheL1DCacheAssociativity = source.CacheL1DCacheAssociativity ?? destination.CacheL1DCacheAssociativity;
            destination.CacheL1DSizeKBytes = source.CacheL1DSizeKBytes ?? destination.CacheL1DSizeKBytes;
            destination.CacheL1DLineSizeInBytes = source.CacheL1DLineSizeInBytes ?? destination.CacheL1DLineSizeInBytes;
            destination.CacheL1DIsShared = source.CacheL1DIsShared ?? destination.CacheL1DIsShared;
            destination.CacheL1DDetails = source.CacheL1DDetails ?? destination.CacheL1DDetails;
            destination.CacheL1ICacheAssociativity = source.CacheL1ICacheAssociativity ?? destination.CacheL1ICacheAssociativity;
            destination.CacheL1ISizeKBytes = source.CacheL1ISizeKBytes ?? destination.CacheL1ISizeKBytes;
            destination.CacheL1ILineSizeInBytes = source.CacheL1ILineSizeInBytes ?? destination.CacheL1ILineSizeInBytes;
            destination.CacheL1IIsShared = source.CacheL1IIsShared ?? destination.CacheL1IIsShared;
            destination.CacheL1IDetails = source.CacheL1IDetails ?? destination.CacheL1IDetails;
            destination.CacheL2CacheAssociativity = source.CacheL2CacheAssociativity ?? destination.CacheL2CacheAssociativity;
            destination.CacheL2SizeKBytes = source.CacheL2SizeKBytes ?? destination.CacheL2SizeKBytes;
            destination.CacheL2LineSizeInBytes = source.CacheL2LineSizeInBytes ?? destination.CacheL2LineSizeInBytes;
            destination.CacheL2IsShared = source.CacheL2IsShared ?? destination.CacheL2IsShared;
            destination.CacheL2Details = source.CacheL2Details ?? destination.CacheL2Details;
            destination.CacheL3CacheAssociativity = source.CacheL3CacheAssociativity ?? destination.CacheL3CacheAssociativity;
            destination.CacheL3SizeKBytes = source.CacheL3SizeKBytes ?? destination.CacheL3SizeKBytes;
            destination.CacheL3LineSizeInBytes = source.CacheL3LineSizeInBytes ?? destination.CacheL3LineSizeInBytes;
            destination.CacheL3IsShared = source.CacheL3IsShared ?? destination.CacheL3IsShared;
            destination.CacheL3Details = source.CacheL3Details ?? destination.CacheL3Details;
            destination.CacheL4CacheAssociativity = source.CacheL4CacheAssociativity ?? destination.CacheL4CacheAssociativity;
            destination.CacheL4SizeKBytes = source.CacheL4SizeKBytes ?? destination.CacheL4SizeKBytes;
            destination.CacheL4LineSizeInBytes = source.CacheL4LineSizeInBytes ?? destination.CacheL4LineSizeInBytes;
            destination.CacheL4IsShared = source.CacheL4IsShared ?? destination.CacheL4IsShared;
            destination.CacheL4Details = source.CacheL4Details ?? destination.CacheL4Details;

            destination.MemoryControllers = source.MemoryControllers ?? destination.MemoryControllers;
            destination.MemoryChannels = source.MemoryChannels ?? destination.MemoryChannels;
            destination.MemoryBandwidthInMbPerSec = source.MemoryBandwidthInMbPerSec ?? destination.MemoryBandwidthInMbPerSec;
            destination.MemoryMaxMemorySizeInMb = source.MemoryMaxMemorySizeInMb ?? destination.MemoryMaxMemorySizeInMb;
            destination.MemoryMemoryType = source.MemoryMemoryType ?? destination.MemoryMemoryType;
            destination.MemoryDetails = source.MemoryDetails ?? destination.MemoryDetails;
            destination.MemoryEccOnly = source.MemoryEccOnly ?? destination.MemoryEccOnly;
            destination.CrystalHeightMm = source.CrystalHeightMm ?? destination.CrystalHeightMm;
            destination.CrystalWidthMm = source.CrystalWidthMm ?? destination.CrystalWidthMm;
            destination.CrystalArea = source.CrystalArea ?? destination.CrystalArea;
            destination.TDP = source.TDP ?? destination.TDP;
            destination.TransistorsCount = source.TransistorsCount ?? destination.TransistorsCount;
            destination.ProcessInNm = source.ProcessInNm ?? destination.ProcessInNm;
            destination.PackageHeightMm = source.PackageHeightMm ?? destination.PackageHeightMm;
            destination.PackageWidthMm = source.PackageWidthMm ?? destination.PackageWidthMm;
            destination.PackageArea = source.PackageArea ?? destination.PackageArea;

            return destination;
        }

        private CpuEntity MapEntity(Cpu cpu)
        {
            return new CpuEntity()
            {
                Model = cpu.Model,
                Name = cpu.Name,
                Manufacturer = cpu.Manufacturer,
                Description = cpu?.Description,
                Cores = cpu.Specs?.Cores,
                Threads = cpu.Specs?.Threads,
                Category = cpu.Category,
                MicroArchitecture = cpu.Specs?.MicroArchitecture,
                InstructionSet = cpu.Specs?.InstructionSet,
                ClockInMhz = cpu.Specs?.ClockInMhz,
                BusInMhz = cpu.Specs?.BusInMhz,
                Multiplier = cpu.Specs?.Multiplier,

                Family = cpu.Family,
                Stepping = cpu.Stepping,
                Revision = cpu.Revision,
                ModelNumber = cpu.ModelNumber,

                CacheL1DCacheAssociativity = cpu?.Cache?.L1Data?.CacheAssociativity,
                CacheL1DSizeKBytes = cpu?.Cache?.L1Data?.SizeKBytes,
                CacheL1DLineSizeInBytes = cpu?.Cache?.L1Data?.LineSizeInBytes,
                CacheL1DIsShared = cpu?.Cache?.L1Data?.IsShared,
                CacheL1DDetails = cpu?.Cache?.L1Data?.Details,

                CacheL1ICacheAssociativity = cpu?.Cache?.L1Instruction?.CacheAssociativity,
                CacheL1ISizeKBytes = cpu?.Cache?.L1Instruction?.SizeKBytes,
                CacheL1ILineSizeInBytes = cpu?.Cache?.L1Instruction?.LineSizeInBytes,
                CacheL1IIsShared = cpu?.Cache?.L1Instruction?.IsShared,
                CacheL1IDetails = cpu?.Cache?.L1Instruction?.Details,

                CacheL2CacheAssociativity = cpu?.Cache?.L2?.CacheAssociativity,
                CacheL2SizeKBytes = cpu?.Cache?.L2?.SizeKBytes,
                CacheL2LineSizeInBytes = cpu?.Cache?.L2?.LineSizeInBytes,
                CacheL2IsShared = cpu?.Cache?.L2?.IsShared,
                CacheL2Details = cpu?.Cache?.L2?.Details,

                CacheL3CacheAssociativity = cpu?.Cache?.L3?.CacheAssociativity,
                CacheL3SizeKBytes = cpu?.Cache?.L3?.SizeKBytes,
                CacheL3LineSizeInBytes = cpu?.Cache?.L3?.LineSizeInBytes,
                CacheL3IsShared = cpu?.Cache?.L3?.IsShared,
                CacheL3Details = cpu?.Cache?.L3?.Details,

                CacheL4CacheAssociativity = cpu?.Cache?.L4?.CacheAssociativity,
                CacheL4SizeKBytes = cpu?.Cache?.L4?.SizeKBytes,
                CacheL4LineSizeInBytes = cpu?.Cache?.L4?.LineSizeInBytes,
                CacheL4IsShared = cpu?.Cache?.L4?.IsShared,
                CacheL4Details = cpu?.Cache?.L4?.Details,

                MemoryControllers = cpu?.MemorySpecs?.Controllers,
                MemoryChannels = cpu?.MemorySpecs?.Channels,
                MemoryBandwidthInMbPerSec = cpu?.MemorySpecs?.BandwidthInMbPerSec,
                MemoryMaxMemorySizeInMb = cpu?.MemorySpecs?.MaxMemorySizeInMb,
                MemoryMemoryType = cpu?.MemorySpecs?.MemoryType,
                MemoryDetails = cpu?.MemorySpecs?.Details,
                MemoryEccOnly = cpu?.MemorySpecs?.EccOnly,

                CrystalHeightMm = cpu?.Crystal?.HeightMm,
                CrystalWidthMm = cpu?.Crystal?.WidthMm,
                CrystalArea = cpu?.Crystal?.Area,

                TDP = cpu?.Crystal?.TDP,
                TransistorsCount = cpu?.Crystal?.TransistorsCount,
                ProcessInNm = cpu?.Crystal?.ProcessInNm,

                PackageHeightMm = cpu?.Package?.HeightMm,
                PackageWidthMm = cpu?.Package?.WidthMm,
                PackageArea = cpu?.Package?.Area,



            };
        }

        public async void Merge(Cpu cpu)
        {
            CpuEntity source = null;

            source = await ReadEntityByIdAsync(cpu.Id);
            if (source == null)
            {
                source = (await ReadEntitiesAsync(new CpuFilter() { SearchString = cpu.Model })).FirstOrDefault();
            }

            var destination = MapEntity(cpu);
            if (source == null)
            {
                try
                {
                    var id = Connection.Insert<CpuEntity>(destination);
                }
                catch (Exception ex)
                {
                    return;
                }
            } else
            {
                destination = MergeEntity(source, destination);

                try
                {
                    var id = Connection.Update<CpuEntity>(destination);
                }
                catch (Exception ex)
                {
                    return;
                }
            }
        }
    }
}