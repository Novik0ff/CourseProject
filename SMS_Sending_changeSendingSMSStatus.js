var Guid = Xrm.Page.data.entity.getId().substr(1, 36);
var result;

var req = new XMLHttpRequest();
req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v8.0/new_smsseltnings(" + Guid + ")", true);
req.setRequestHeader("OData-MaxVersion", "4.0");
req.setRequestHeader("OData-Version", "4.0");
req.setRequestHeader("Accept", "application/json");
req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
req.onreadystatechange = function() {
    if (this.readyState === 4) {
        req.onreadystatechange = null;
        if (this.status === 200) {
            result = JSON.parse(this.response);
			if(result.new_message_status_code != 1){
				result.new_message_status_code = 1;
			}
			Xrm.Page.getAttribute("new_message_status_code").setValue(result.new_message_status_code);
        } else {
            Xrm.Utility.alertDialog(this.statusText);
        }
    }
};
req.send();
Xrm.Page.data.save();