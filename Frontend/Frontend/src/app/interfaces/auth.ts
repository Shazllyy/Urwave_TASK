export interface RegisterPostData {
  fullName: string;
  email: string;
  password: string;
}

export interface User extends RegisterPostData {
  id: string;
}

export interface LoginDTO {
  username: string;
  password: string;
}

export interface AuthResponseDTO {
  token: string;
  antiForgeryToken:string;
}