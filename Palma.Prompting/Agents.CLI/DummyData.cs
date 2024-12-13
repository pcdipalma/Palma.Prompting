namespace Agents.CLI
{
    public static class DummyData
    {
        public static Dictionary<string, List<CalendarSlot>> CalendarData { get; set; } = new Dictionary<string, List<CalendarSlot>>
        {
            { "Alice", new List<CalendarSlot> { new CalendarSlot("9:00 AM", "10:00 AM"), new CalendarSlot("1:00 PM", "2:00 PM"), new CalendarSlot("3:00 PM", "5:00 PM") } },
            { "Bob", new List<CalendarSlot> { new CalendarSlot("10:00 AM", "12:00 PM"), new CalendarSlot("2:00 PM", "3:00 PM") } },
            { "Charlie", new List<CalendarSlot> { new CalendarSlot("9:00 AM", "11:00 AM"), new CalendarSlot("1:00 PM", "3:00 PM") } },
            { "Diana", new List<CalendarSlot> { new CalendarSlot("11:00 AM", "12:00 PM"), new CalendarSlot("4:00 PM", "5:00 PM") } },
            { "Eve", new List<CalendarSlot> { new CalendarSlot("9:00 AM", "10:00 AM"), new CalendarSlot("2:00 PM", "4:00 PM") } }
        };

        public static Dictionary<string, ProjectDetails> ProjectData { get; set; } = new Dictionary<string, ProjectDetails>
        {
            { "Project Alpha", new ProjectDetails("Project Alpha", "In Progress", new List<Person>
                {
                    new Person("Alice", "Project Manager"),
                    new Person("Bob", "Developer"),
                    new Person("Charlie", "Tester")
                })
            },
            { "Project Beta", new ProjectDetails("Project Beta", "Completed", new List<Person>
                {
                    new Person("Diana", "Project Manager"),
                    new Person("Eve", "Developer"),
                    new Person("Alice", "Tester")
                })
            },
            { "Project Gamma", new ProjectDetails("Project Gamma", "Not Started", new List<Person>
                {
                    new Person("Bob", "Project Manager"),
                    new Person("Charlie", "Developer"),
                    new Person("Diana", "Tester")
                })
            },
            { "Project Delta", new ProjectDetails("Project Delta", "In Progress", new List<Person>
                {
                    new Person("Eve", "Project Manager"),
                    new Person("Alice", "Developer"),
                    new Person("Bob", "Tester")
                })
            },
            { "Project Epsilon", new ProjectDetails("Project Epsilon", "Completed", new List<Person>
                {
                    new Person("Charlie", "Project Manager"),
                    new Person("Diana", "Developer"),
                    new Person("Eve", "Tester")
                })
            }
        };
    }
}
