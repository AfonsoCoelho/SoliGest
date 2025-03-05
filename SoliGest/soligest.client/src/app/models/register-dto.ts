export class RegisterDto {
  userName: string;
  email: string;
  password: string;
  phoneNumber: string;
  address: string;

  constructor(userName: string, email: string, password: string, phoneNumber: string, address: string) {
    this.userName = userName;
    this.email = email;
    this.password = password;
    this.phoneNumber = phoneNumber;
    this.address = address;
  }
}
