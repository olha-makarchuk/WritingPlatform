import { Component, OnDestroy, OnInit } from '@angular/core';
import { TokenStorageService } from '../_services/token-storage.service';
import { UserService } from '../_services/user.service';
import { PersonalInformationChange } from '../shared/models/personal-informatin-change';
import { EventBusService } from '../_services/event-bus.service';
import { Subscription } from "rxjs";

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit , OnDestroy {
  isLoggedIn = false;
  username?: string;
  eventBusSub?: Subscription;
  currentUser: any = {};
  userId: string = ''; 

  constructor(private token: TokenStorageService, private eventBusService: EventBusService, private userService: UserService) { }
  
  ngOnInit(): void {
    this.loadUserInfo();
  }

  loadUserInfo(): void {
    const userName = this.token.getUser().userName;
    this.userId = this.token.getUser().userId;
    this.userService.getUserByUserName(this.userId).subscribe(
      data => {
        this.currentUser = data;
      },
      err => {
        console.error(err);
      }
    );
  }

  updateUserInfo(): void {
    const { userName, firstName, lastName, personalInformation , email} = this.currentUser;
    this.userService.changeUserInfoByUserName(this.userId, userName, lastName, firstName, personalInformation).subscribe(
      (response: PersonalInformationChange) => {
        alert('User information updated successfully');
        const updatedUser = { ...this.token.getUser(), userName };
        this.token.saveUser(updatedUser);
        this.loadUserInfo(); 
      },
      err => {
        console.error(err);
        alert('Failed to update user information');
      }
    );
  }

  ngOnDestroy(): void {
    if (this.eventBusSub)
      this.eventBusSub.unsubscribe();
  }

  onDeleteAccount(): void {
    this.userService.deleteAccount(this.userId).subscribe();
    this.token.signOut();
    this.isLoggedIn = false;
  }

}
