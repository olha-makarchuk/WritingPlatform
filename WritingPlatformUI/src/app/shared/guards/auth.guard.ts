import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { AuthService } from "../../_services/auth.service";
import { TokenStorageService } from "../../_services/token-storage.service";

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate  {
  constructor(private tokenStorageService: TokenStorageService, private authService: AuthService, private router: Router) {}

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (!!this.tokenStorageService.getToken()) {
      return true;
    } else {
      this.router.navigate(["/login"]);
      return false;
    }
  }
}
