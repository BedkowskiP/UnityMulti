
let Users = {

};
const AddUser = async (content,isValid,socket) =>
{
    Users[content.UserID]= new User(content.UserID,content.Username,isValid,socket);
}
const RemoveUser = async (UserID) =>
{
    try
    {
        await delete Users[UserID];
    }
    catch(e)
    {
        console.log("coudlnt remove User")
    }
}



class User
{
    constructor(id,name,valid,socket)
    {
        this.id=id,
        this.name=name,
        this.valid=valid,
        this.socket=socket
    } 
}

module.exports={AddUser,User,Users,RemoveUser}



