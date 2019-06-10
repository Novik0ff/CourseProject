function changeMailingStatus(){
	Xrm.Page.getAttribute("statuscode").setValue(100000000);
	Xrm.Page.data.save();
	
	function updatePage(){
		var guid = Xrm.Page.data.entity.getId().substr(1,36);
		var req = new XMLHttpRequest();
		req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v8.0/new_smsseltnings(" + guid + ")?$select=statuscode", true);
		req.setRequestHeader("OData-MaxVersion", "4.0");
		req.setRequestHeader("OData-Version", "4.0");
		req.setRequestHeader("Accept", "application/json");
		req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
		req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
		req.onreadystatechange = function() {
		    if (this.readyState !== 4) {
					return;
				}
        req.onreadystatechange = null;
        if (this.status !== 200) {
					Xrm.Utility.alertDialog(this.statusText);
				}
        var result = JSON.parse(this.response);
				Xrm.Page.getAttribute("statuscode").setValue(result["statuscode"]);
		};
		req.send();
		Xrm.Page.getAttribute("statuscode").setValue(statuscode);
	}
	setTimeout(updatePage, 10000);
}