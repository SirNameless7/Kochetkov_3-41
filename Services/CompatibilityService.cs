using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KPO_Cursovoy.Models;

namespace KPO_Cursovoy.Services
{
    public class CompatibilityService
    {
        public Task<CompatibilityResult> CheckCompatibilityAsync(List<ComponentItem> components)
        {
            var result = new CompatibilityResult { IsCompatible = true };

            ComponentItem? cpu = components.FirstOrDefault(c => c.CategoryCode == "CPU");
            ComponentItem? mb = components.FirstOrDefault(c => c.CategoryCode == "MB");
            ComponentItem? gpu = components.FirstOrDefault(c => c.CategoryCode == "GPU");
            ComponentItem? psu = components.FirstOrDefault(c => c.CategoryCode == "PSU");
            ComponentItem? pcCase = components.FirstOrDefault(c => c.CategoryCode == "CASE");

            var rams = components.Where(c => c.CategoryCode == "RAM").ToList();
            var ssds = components.Where(c => c.CategoryCode == "SSD").ToList();

            // CPU <-> MB (Socket)
            if (cpu != null && mb != null && !string.IsNullOrWhiteSpace(cpu.Socket) && !string.IsNullOrWhiteSpace(mb.Socket))
            {
                if (!string.Equals(cpu.Socket, mb.Socket, System.StringComparison.OrdinalIgnoreCase))
                    Add(result, cpu.Name, mb.Name, $"Разные сокеты: CPU={cpu.Socket}, MB={mb.Socket}");
            }

            // MB <-> RAM (тип памяти)
            if (mb != null && rams.Count > 0 && !string.IsNullOrWhiteSpace(mb.MemoryType))
            {
                foreach (var ram in rams)
                {
                    if (!string.IsNullOrWhiteSpace(ram.MemoryType) &&
                        !string.Equals(ram.MemoryType, mb.MemoryType, System.StringComparison.OrdinalIgnoreCase))
                    {
                        Add(result, mb.Name, ram.Name, $"Тип памяти не совпадает: MB={mb.MemoryType}, RAM={ram.MemoryType}");
                    }
                }

                // количество планок <= слотов (если известно)
                if (mb.RamSlots.HasValue && rams.Count > mb.RamSlots.Value)
                    Add(result, mb.Name, "RAM", $"Слишком много модулей RAM: {rams.Count}, слотов на MB: {mb.RamSlots.Value}");
            }

            // GPU <-> CASE (длина)
            if (gpu != null && pcCase != null && gpu.GpuLengthMm.HasValue && pcCase.MaxGpuLengthMm.HasValue)
            {
                if (gpu.GpuLengthMm.Value > pcCase.MaxGpuLengthMm.Value)
                    Add(result, gpu.Name, pcCase.Name, $"Видеокарта длиннее корпуса: GPU={gpu.GpuLengthMm}мм, CASE max={pcCase.MaxGpuLengthMm}мм");
            }

            // PSU (мощность)
            if (psu != null && psu.Wattage.HasValue)
            {
                int baseW = 60; // материнка/вентиляторы/периферия
                int ramW = rams.Count * 5;
                int ssdW = ssds.Count * 5;

                int cpuW = cpu?.PowerDrawW ?? 0;
                int gpuW = gpu?.PowerDrawW ?? 0;

                // запас 30%
                int required = (int)System.Math.Ceiling((baseW + ramW + ssdW + cpuW + gpuW) * 1.3);

                if (psu.Wattage.Value < required)
                    Add(result, psu.Name, "Система", $"Не хватает мощности БП: PSU={psu.Wattage}W, нужно примерно {required}W");
            }

            return Task.FromResult(result);
        }

        private static void Add(CompatibilityResult result, string c1, string c2, string reason)
        {
            result.IsCompatible = false;
            result.IncompatiblePairs.Add(new IncompatiblePair
            {
                Component1 = c1,
                Component2 = c2,
                Reason = reason
            });
        }
    }

    public class CompatibilityResult
    {
        public bool IsCompatible { get; set; }
        public List<IncompatiblePair> IncompatiblePairs { get; set; } = new();
    }

    public class IncompatiblePair
    {
        public string Component1 { get; set; } = string.Empty;
        public string Component2 { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
