import { Component, OnInit } from '@angular/core';
import { Publication } from '../../shared/models/publication';
import { ActivatedRoute } from '@angular/router';
import { UserService } from '../../_services/user.service';
import { TokenStorageService } from '../../_services/token-storage.service';

@Component({
  selector: 'app-my-publications',
  templateUrl: './my-publications.component.html',
  styleUrls: ['./my-publications.component.css']
})
export class MyPublicationsComponent implements OnInit {
  publications: Publication[] = [];
  isLoggedIn = false;
  userId: string = "";
  currentPage: number = 1;
  pageSize: number = 3; 
  totalPages: number = 0;

  constructor(
    private tokenStorageService: TokenStorageService,
    private route: ActivatedRoute,
    private userService: UserService
  ) { }
  
  ngOnInit(): void {
    this.isLoggedIn = !!this.tokenStorageService.getToken();
    
    const user = this.tokenStorageService.getUser();
    if (user) {
      this.userId = user.userId || null;
    }

    this.fetchPublications();
  }

  fetchPublications(): void {
    this.userService.getPublicationsByAuthor(this.userId, this.currentPage, this.pageSize).subscribe(publications => {
      this.publications = publications;
      this.totalPages = publications[0].paginatorCount;
    });
  }

  onDeletePublication(publicationId: number): void {
    this.userService.deletePublication(publicationId).subscribe(() => {
      this.fetchPublications();
    });
  }

  onPageChange(pageNumber: number): void {
    this.currentPage = pageNumber;
    this.fetchPublications();
  }

  getPaginatedPublications(): Publication[] {
    return this.publications;
  }
}
