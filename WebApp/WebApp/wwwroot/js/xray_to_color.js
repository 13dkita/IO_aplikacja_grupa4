let loaded = [];

$(".roentgenColorBtn").each((i, button) => {
    loaded.push(false);

    $(button).on("click", () => {
        
        if (loaded.length == 0 || loaded[i] != true){
            loaded[i] = true;

            $(`#roentgenImageColor${i}`).hide();
            $(`#imageLoadingSpinner${i}`).show();

            $.ajax({
                type: "GET",
                url: `https://localhost:44302/api/RoentgenImage/${$("#patientIdValue" + i).attr("value")}`
            }).done((data) => {
                $(`#roentgenImageColor${i}`).attr("src", data);
                $(`#roentgenImageColor${i}`).show()
                $(`#imageLoadingSpinner${i}`).hide();
            });
        }
    });
});
