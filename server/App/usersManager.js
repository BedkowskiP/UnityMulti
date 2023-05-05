
let Users = {

};
const signChecker = async (stringToCheck) => {
    const regex = /^[0-9a-zA-Z_]+$/;
    return regex.test(stringToCheck);
}
const createUserId = async () => {
    const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let userId = '';
    let unique = false;
    while(!unique)
    {
        unique = true
        userId = '';
        for (let i = 0; i < 16; i++) {
            const randomIndex = Math.floor(Math.random() * chars.length);
            userId += chars[randomIndex];
        
        }
        for(let key in Users)if(key==userId)unique=false;
    }
    return userId;
};
const CreateUser = async (socket) =>
{
    try
    {
        const UserID = await createUserId()
        Users[UserID] = new User(UserID,socket);
        return UserID;
    }
    catch
    {
        
        return 400;//errorcode
    }
    
}
const RemoveUser = async (UserID) =>
{
    try
    {
        await delete Users[UserID];
    }
    catch(e)
    {
        return 300; //300 errorcode - couldnt delete user from Users list
        //console.log("coudlnt remove User")
    }
}
class User
{
    constructor(id,socket)
    {
        this._id=id,//ID to differentiate users
        this._name=null,//username to represent users on client side
        this._valid=null,//validated guest vs verifed user
        this._socket=socket//reference to socket
    } 
    get id() {return this._id;}
    get name() {return this._name;}
    get valid() {return this._valid;}
    get socket() {return this._socket;}
      

    set id(id) 
    {
        this._id = id;
    }

    SetName(NAME)
    {
        if(NAME==="")this._name = this._id;//if username empty id becomes username
        else if(typeof NAME==="string")//if username is at least string check with regex and change user.name=Username
        {
            if(signChecker(NAME))this._name = NAME
            
            else return 401;
        }
        else return 401;
        this._valid=true;// Change it to valid true at set password later on
        return 0;
    }

    set valid(valid) {this._valid = valid;}

    set socket(socket) {this._socket = socket;}
}

module.exports={User,Users,RemoveUser,CreateUser}



