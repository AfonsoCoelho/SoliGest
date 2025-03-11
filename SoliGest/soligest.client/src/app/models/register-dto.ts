export class RegisterDto {
  userName: string;
  email: string;
  password: string;
  phoneNumber: string;
  address: string;
  birthDate: Date;

  constructor(userName: string, email: string, password: string, phoneNumber: string, address: string, birthDate: Date) {
    this.userName = userName;
    this.email = email;
    this.password = password;
    this.phoneNumber = phoneNumber;
    this.address = address;
    this.birthDate = birthDate;
  }
}
