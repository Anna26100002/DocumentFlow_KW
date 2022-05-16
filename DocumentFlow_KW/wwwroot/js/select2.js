$("#executor").select2({
    placeholder: "Выберите исполнителя",
    theme: "bootstrap5",
    allowClear: true
    /*data: JSON.parse('@Html.Raw(Json.Serialize(Model.Task.Executor))')*/
});

$('#executor').val();