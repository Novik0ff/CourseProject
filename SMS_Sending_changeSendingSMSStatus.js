function changeMailingStatus(){
	Xrm.Page.getAttribute("statuscode").setValue(100000000);
	Xrm.Page.data.save();
}


