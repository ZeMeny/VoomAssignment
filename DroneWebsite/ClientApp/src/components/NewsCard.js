import Card from '@material-ui/core/Card';
import CardActionArea from '@material-ui/core/CardActionArea';
import CardActions from '@material-ui/core/CardActions';
import CardContent from '@material-ui/core/CardContent';
import CardMedia from '@material-ui/core/CardMedia';
import Typography from '@material-ui/core/Typography';
import Link from '@material-ui/core/Link';
import React, { Component } from 'react';

function goToUrl(url){
    window.open(url, '_blank').focus();
}    

function extractDate(dateString){
    var date = new Date(dateString);
    return `From ${date.getFullYear()}-${date.getMonth()}-${date.getDate()}`
}

async function getAuthorUrl(name) {
    const response = await fetch(`https://${window.location.host}/api/articles/author?name=${name}`);
    if (response.status == 200){
        const url = await response.text();
        if (url){
            return url;
        }
    }
}

export default class MediaCard extends Component {
    constructor(props) {
        super(props);
        this.state = {
            authorUrl: ""
          };
    }
    
    render() {
        getAuthorUrl(this.props.article.author).then(value=>{
            this.setState({authorUrl: value})
        });
        
        return (
            <Card className="card" style={{maxWidth: 345, margin:10}}>
                <CardActionArea onClick={()=>goToUrl(this.props.article.url)}>
                    <CardMedia 
                        className="media"
                        style={{height: 140}}
                        image={this.props.article.urlToImage}
                        title={ this.props.article.author }
                    />
                    <CardContent>
                        <Typography gutterBottom variant="h5" component="h2">
                           { this.props.article.title }
                        </Typography>
                        <Typography variant="body2" color="textSecondary" component="p">
                            {this.props.article.content.replace(/\[.+\]/g, '')}
                        </Typography>
                    </CardContent>
                </CardActionArea>
                <CardActions>
                    <Typography size="small">
                        {extractDate(this.props.article.publishedAt)}
                    </Typography>
                    <Link href={this.state.authorUrl}>
                        About the Author
                    </Link>
                </CardActions>
            </Card>
        );
    }
}
