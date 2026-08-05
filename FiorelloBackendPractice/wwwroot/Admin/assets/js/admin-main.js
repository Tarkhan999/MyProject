"use strict";

let deleteBtns=document.querySelectorAll(".delete-btn");
deleteBtns.forEach(btn=>{
    btn.addEventListener("click",function(){
       let id=parseInt(this.getAttribute("data-id"));
       fetch(`/admin/category/delete/${id}`,{
           method:"Post",
           headers:{"Content-Type":"application/json"}
       }).then(response=>{
           if(response.ok){
              btn.parentNode.parentNode.remove();
           }
           else{
               alert("Failed to Delete!");
           }
       })
    });
});