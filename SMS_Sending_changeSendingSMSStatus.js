function changeMailingStatus(){
	Xrm.Page.getAttribute("new_message_status_code").setValue(1);
	Xrm.Page.data.entity.save();
}


