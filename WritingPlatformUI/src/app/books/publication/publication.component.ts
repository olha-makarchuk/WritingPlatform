import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Publication } from '../../shared/models/publication';
import { UserService } from "../../_services/user.service";
import { TokenStorageService } from '../../_services/token-storage.service';
import { Comments } from '../../shared/models/get-comment';
import { Author } from '../../shared/models/author';

@Component({
  selector: 'app-publication',
  templateUrl: './publication.component.html',
  styleUrls: ['./publication.component.css']
})
export class PublicationComponent implements OnInit {
  publication: Publication | null = null;
  author: Author | null = null;
  comments: Array<Comments> = [];
  isLoggedIn = false;
  login: string = '';
  newCommentText: string = '';
  loggedInUserId: string = '';

  constructor(
    private tokenStorageService: TokenStorageService,
    private route: ActivatedRoute,
    private userService: UserService
  ) { }

  activeTab: string = 'book';

  showBookInfo() {
    this.activeTab = 'book';
  }

  showAuthorInfo() {
    this.activeTab = 'author';
  }

  showReviewsInfo() {
    this.activeTab = 'reviews';
  }

  ngOnInit(): void {
    this.isLoggedIn = !!this.tokenStorageService.getToken();
    
    const user = this.tokenStorageService.getUser();
    if (user) {
      this.loggedInUserId = user.userId || null;
      this.login = user.userId || null;
    }

    const publicationId = this.route.snapshot.params['publicationId'];

    this.userService.getPublicationById(publicationId).subscribe(publication => {
      this.publication = publication;
    });

    this.userService.getComments(publicationId).subscribe(comments => {
      this.comments = comments;
    });
  }
  
  onSubmitComment(): void {
    if (this.newCommentText.trim() && this.publication) {
      this.userService.createComment(this.publication.publicationId, this.newCommentText, this.login).subscribe(() => {
        this.userService.getComments(this.publication!.publicationId).subscribe(comments => {
          this.comments = comments;
          this.newCommentText = '';
        });
      });
    }
  }

  onDeleteComment(commentId: number): void {
    this.userService.deleteComment(commentId).subscribe(() => {
      this.comments = this.comments.filter(comment => comment.commentId !== commentId);
    });
  }
  
}
