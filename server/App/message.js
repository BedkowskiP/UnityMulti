const CreateMsg =  (type,cont,err,log,broadcast) =>
{
    let result = 
    {
        Type : type,
        Content : JSON.stringify(cont),
        ErrorCode : err, 
        Timestamp: Date.now()
    }
    
    if(log&&broadcast==null)console.log("msg from server: ",{Type : type,Content : cont,ErrorCode : err,Timestamp: Date.now()})
    return result
}

module.exports={CreateMsg}



