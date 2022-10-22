import { Button, Stack, Typography } from "@mui/material";
import React from "react";

interface IProps {
  setHavePrototype: React.Dispatch<React.SetStateAction<Boolean | undefined>>;
}

export default function SelectApproach(props: IProps) {
  return (
    <div>
      <Stack spacing={4}>
        <Typography variant="h1" sx={{ fontSize: "1.8rem", mt: "10px" }}>
          Welcome to <span style={{ fontWeight: "bold" }}>Table to Code</span>{" "}
          program
        </Typography>
        <Typography variant="body1" sx={{ marginTop: "32px !important" }}>
          Do you have the table prototype information (like the table below)?
        </Typography>
        <pre
          style={{
            whiteSpace: "pre-wrap",
            marginRight: "auto",
            marginLeft: "auto",
            backgroundColor: "#f5f2f0",
          }}
        >
          +-------------+---------+<br />
          | Column Name | Type    |<br />
          +-------------+---------+<br />
          | project_id  | int     |<br />
          | employee_id | int     |<br />
          +-------------+---------+<br />
        </pre>
        <Stack direction="row" spacing={2} sx={{ justifyContent: "center" }}>
          <Button
            variant="outlined"
            sx={{ width: "120px" }}
            onClick={() => props.setHavePrototype(true)}
          >
            Yes
          </Button>
          <Button
            variant="outlined"
            sx={{ width: "120px" }}
            onClick={() => props.setHavePrototype(false)}
          >
            No
          </Button>
        </Stack>
      </Stack>
    </div>
  );
}
