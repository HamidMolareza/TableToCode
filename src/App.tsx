import { Box, Container, Paper } from "@mui/material";
import React, { useState } from "react";
import SelectApproach from "./SelectApproach";

export default function App() {
  const [havePrototype, setHavePrototype] = useState<Boolean | undefined>(
    undefined
  );
  console.log(havePrototype);

  return (
    <Box
      sx={{
        minWidth: "100%",
        minHeight: "100vh",
        backgroundColor: "grey.200",
        display: "flex",
        alignItems: "center",
      }}
    >
      <Container>
        <Paper
          sx={{ minHeight: "500px", padding: "15px", textAlign: "center" }}
        >
          <SelectApproach setHavePrototype={setHavePrototype} />
        </Paper>
      </Container>
    </Box>
  );
}
