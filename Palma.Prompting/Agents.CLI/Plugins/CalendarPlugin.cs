using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Agents.CLI
{
    public class CalendarPlugin
    {
        [KernelFunction("check_available_days")]
        [Description("Checks for available days for meetings for a list of users")]
        [return: Description("A list of available calendar slots where all users are free")]
        public Task<List<CalendarSlot>> CheckAvailableDaysAsync(List<string> names)
        {
            var availableSlots = new List<CalendarSlot>();

            foreach (var slot in DummyData.CalendarData.Values.SelectMany(slots => slots).Distinct())
            {
                if (names.All(name => DummyData.CalendarData.ContainsKey(name) && DummyData.CalendarData[name].Contains(slot)))
                {
                    availableSlots.Add(slot);
                }
            }

            return Task.FromResult(availableSlots);
        }

        [KernelFunction("schedule_meeting")]
        [Description("Schedules a meeting on a specified day")]
        [return: Description("Confirmation of the scheduled meeting")]
        public Task<string> ScheduleMeetingAsync(string day)
        {
            // Dummy implementation - replace with actual logic
            return Task.FromResult($"Meeting scheduled on {day}");
        }
    }

    public class CalendarSlot
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public CalendarSlot(string startTime, string endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public override bool Equals(object obj)
        {
            if (obj is CalendarSlot slot)
            {
                return StartTime == slot.StartTime && EndTime == slot.EndTime;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StartTime, EndTime);
        }
    }
}
