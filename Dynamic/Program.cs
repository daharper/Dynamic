using Dynamic.Runtime;
using Dynamic.Career;

ActiveRuntime.Register(typeof(Person));

var david = new Person
{
    Name = "David Harper",
    Location = "Spennymoor, County Durham",
    Availability = Availability.Immediately,
    WorkingArrangements = WorkingArrangement.Remote | WorkingArrangement.Hybrid,
    MinimumSalary = new Money(50_000m, "GBP")
};

david.RightToWork.Australia();
david.RightToWork.UnitedKingdom();
david.RightToWork.Ireland();

david.Achievements.Mvp("Embarcadero");

david.Show();
