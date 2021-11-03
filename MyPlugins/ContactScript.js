var Sdk = window.Sdk || {};
(
    function () {
        this.formOnLoad = function (executionContext) {
            var formContext = executionContext.getFormContext();
            var firstName = formContext.getAttribute("firstname").getValue();
            alert("Hello: " + firstName);
        }
    }

).call(Sdk);