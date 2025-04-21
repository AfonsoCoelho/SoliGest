// login and register
export interface UserDto {
  email: string;
  password: string;
}

// manage/info
export interface UserInfo {
  id: string;
  name: string;
  email: string;
  isEmailConfirmed: boolean;
}
