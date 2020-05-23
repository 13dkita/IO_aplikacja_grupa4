document.addEventListener("DOMContentLoaded", function () {
  const calendarEl = document.getElementById("calendar");

  let calendar = new FullCalendar.Calendar(calendarEl, {
    timeZone: "local",
    plugins: ["dayGrid", "timeGrid", "list"],
    header: { center: "dayGridMonth,timeGridDay,list,addEventButton" },
    buttonText: {
      dayGridMonth: "Miesiąc",
      timeGridDay: "Dzień",
      list: "Lista",
    },
    customButtons: {
      addEventButton: {
        text: "Dodaj wizytę",
        click: function () {
          $("#addEventModal").modal("show");
        },
      },
    },
    locale: "pl",
  });

  $.ajax({
    type: "GET",
    url: "https://localhost:44302/api/Timetable",
  }).done((events) => {

    events.forEach((event) => {
      calendar.addEvent({
        title: `${event.patient.firstName} ${event.patient.lastName}`,
        start: event.start,
        end: event.end,
      });
    });

    calendar.render();
  });
});
