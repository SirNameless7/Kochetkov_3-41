using System;
using System.Collections.Generic;

namespace KPO_Cursovoy.Models
{
    public class ComponentItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }

        // --- ДОБАВЛЕНО ДЛЯ СОВМЕСТИМОСТИ ---

        // CPU/MB
        public string? Socket { get; set; }

        // RAM/MB
        public string? MemoryType { get; set; }   // "DDR4", "DDR5"
        public int? RamSlots { get; set; }        // только для MB

        // GPU/CASE
        public int? GpuLengthMm { get; set; }     // только для GPU
        public int? MaxGpuLengthMm { get; set; }  // только для CASE

        // PSU/Power
        public int? Wattage { get; set; }         // только для PSU
        public int? PowerDrawW { get; set; }      // CPU/GPU (примерное потребление)
    }
}
