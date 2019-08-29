import * as React from "react";
import { loadGraph, resetGraph } from "./graph-view.business";
import { withTheme, WithTheme } from "material-ui/styles";
import {
  GraphApi,
  CreateGraphApi,
  GraphConfig,
  GraphResponse
} from "../../../../graph-api";
import { jfkServiceConfig } from "../../service/jfk";
import { cnc } from "../../../../util";
import { Select, MenuItem } from 'material-ui';

const style = require("./graph-view.style.scss");


interface GraphViewProps extends WithTheme {
  searchValue: string;
  facet: string;
  graphConfig?: GraphConfig;
  onGraphNodeDblClick : (searchValue : string) => string;
  className?: string;
}

interface GraphViewState {
  graphApi: GraphApi;
  graphDescriptor: GraphResponse;
  facet: string;
}

const containerId = "fdGraphId";

class GraphView extends React.Component<GraphViewProps, GraphViewState> {


  constructor(props) {
    super(props);
  
    this.state = {
      graphApi: CreateGraphApi(jfkServiceConfig.graphConfig),
      graphDescriptor: null,
      facet: props.facet
    }
  }

  private fetchGraphDescriptor = async (searchValue: string, facet: string) => {
    if (!this.state.graphApi || !searchValue) return Promise.resolve(null);

    try {
      const payload = {search: searchValue, facet: facet};
      return await this.state.graphApi.runQuery(payload);
    } catch (e) {
      throw e;
    }
  };

  private updateGraphDescriptor = (searchValue: string, facet:string) => {
    this.fetchGraphDescriptor(searchValue, facet)
      .then(graphDescriptor => this.setState({
        ...this.state,
        graphDescriptor, 
        facet
      }))
      .catch(e => console.log(e));
  }

  private updateGraphApiAndDescriptor = (graphConfig: GraphConfig, searchValue: string, facet: string) => {
    this.setState({
      ...this.state,
      graphApi: CreateGraphApi(graphConfig || jfkServiceConfig.graphConfig),
      facet
    }, () => this.updateGraphDescriptor(searchValue, facet));
  }

  public componentDidMount() {
    this.updateGraphApiAndDescriptor(this.props.graphConfig, this.props.searchValue, 'anatomical_site_concepts');
  };

  public componentWillReceiveProps(nextProps: GraphViewProps) {
    if (this.props.searchValue != nextProps.searchValue) {
      this.updateGraphDescriptor(nextProps.searchValue, nextProps.facet);
    } else if (this.props.graphConfig != nextProps.graphConfig) {
      this.updateGraphApiAndDescriptor(nextProps.graphConfig, nextProps.searchValue, nextProps.facet);
    }
  }

  public shouldComponentUpdate(nextProps: GraphViewProps, nextState: GraphViewState) {
    return this.state.graphDescriptor != nextState.graphDescriptor
  }

  public componentDidUpdate(prevProps: GraphViewProps, prevState: GraphViewState) {
    if (this.state.graphDescriptor != prevState.graphDescriptor) {
      loadGraph(containerId, this.state.graphDescriptor, this.props.onGraphNodeDblClick, this.props.theme);
    }      
  }

  public componentWillUnmount() {
    resetGraph(containerId);
  }

  handleChange = event => {
    this.updateGraphApiAndDescriptor(this.props.graphConfig, this.props.searchValue,  event.target.value);
  };

  public render() {
    return (
     <span>
        <Select className={style.select} 
        value={this.state.facet}
        onChange={this.handleChange}
      >
        <MenuItem value={"anatomical_site_concepts"}>Anatomical Concepts</MenuItem>
        <MenuItem value={"anatomical_sites"}>Anatomical Terms</MenuItem>
        <MenuItem value={"disease_disorder_concepts"}>Disease & Disorder Concepts</MenuItem>
        <MenuItem value={"disease_disorders"}>Disease & Disorder Terms</MenuItem>
        <MenuItem value={"medical_mention_concepts"}>Medication Concepts</MenuItem>
        <MenuItem value={"medical_mentions"}>Medication Terms</MenuItem>
        <MenuItem value={"organizations"}>Organizations</MenuItem>
        <MenuItem value={"people"}>People</MenuItem>
        <MenuItem value={"sign_symptom_concepts"}>Sign & Symptoms Concepts</MenuItem>
        <MenuItem value={"sign_symptoms"}>Sign & Symptoms Terms</MenuItem>
      </Select>
        <div
          className={cnc(style.container, this.props.className)}
          id={containerId}
        >
        </div>
      </span>
    );
  }
}

export const GraphViewComponent = withTheme()(GraphView);