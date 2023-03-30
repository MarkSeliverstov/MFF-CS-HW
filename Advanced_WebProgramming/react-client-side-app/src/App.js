import './App.css';
import React from 'react';
import { Form, Button, InputGroup, Container, Nav, Navbar } from 'react-bootstrap';

function NavBar(props) {
  return (
    <div className="NavBar">
      <Navbar bg="dark" variant="dark">
        <Container>
          <Navbar.Brand>SimpleApp</Navbar.Brand>
          <Nav className="me-auto">
            <Nav.Link onClick={() => props.setPageIndex(0)} href="#Home">Home</Nav.Link>
            <Nav.Link onClick={() => props.setPageIndex(1)} href="#Increase">Increase</Nav.Link>
            <Nav.Link onClick={() => props.setPageIndex(2)} href="#Decrease">Decrease</Nav.Link>
          </Nav>
        </Container>
      </Navbar>
    </div>
  );
}

function ClearLabel() {
  document.querySelector('.label').textContent = '';
  document.querySelector('.form-control').value = '';
}

function MainPage() {
  return (
    <div className="page">
      <h1 className="label">SimpleApp</h1>
      <InputGroup className="mb-3">
        <Form.Control
          placeholder="Edit label"
          aria-label="Edit label"
          aria-describedby="basic-addon2"
          onChange={(event)=>document.querySelector('.label').textContent = event.target.value}
        />
        <Button onClick={ClearLabel} variant="outline-secondary" id="button-addon2">
          clear
        </Button>
      </InputGroup>
    </div>
  );
}

function IncreasePage(props) {
  return (
    <div className="page">
      <h1>Increase</h1>
      <p>Counter value: {props.counter}</p>
      <Button onClick={() => props.setCounter(props.counter + 1)} variant="outline-secondary" id="button-addon2">
        Increase
      </Button>
    </div>
  );
}

function DecreasePage(props) {
  return (
    <div className="page">
      <h1>Decrease</h1>
      <p>Counter value: {props.counter}</p>
      <Button onClick={() => props.setCounter(props.counter - 1)} variant="outline-secondary" id="button-addon2">
        Decrease
      </Button>
    </div>
  );
}


function App() {
  const [pageIndex, setPageIndex] = React.useState(0);
  const [counter, setCounter] = React.useState(0);
  const pages = [MainPage, () => <IncreasePage counter={counter} setCounter={setCounter} />, () => <DecreasePage counter={counter} setCounter={setCounter} />];
  const CurrentPage = pages[pageIndex];

  return (
    <div className="App">
      <NavBar setPageIndex={setPageIndex} />
      <CurrentPage />
    </div>
  );
}


export default App;
