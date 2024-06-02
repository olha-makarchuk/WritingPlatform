import { TokenApi } from "./token-api";

export interface Login {
  userId: string;
  userName: string;
  token: string;
  tokenApi: TokenApi;
}
